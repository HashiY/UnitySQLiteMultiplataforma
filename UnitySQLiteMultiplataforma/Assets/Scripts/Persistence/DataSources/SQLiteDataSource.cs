using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.Networking;
using System;
using Assets.Scripts.Persistence.DAO.Specification;

public class SQLiteDataSource : MonoBehaviour, ISQLiteConnectionProvider
{
    public string databaseName; // nome do arquivo
    protected string databasePath;//caminho para o bd              
    public SqliteConnection Connection => new SqliteConnection($"Data Source = {this.databasePath};");

    [SerializeField]
    protected bool copyDatabase;

    public static Action registerInitialLoad;//delegate void system Action

    //o const nao pode ser alterado demtro do codigo
    private const int codeVersion = 1; 
    private const string tagVersion = "DB_VERSION"; 

    protected void Awake()
    {
        var game = FindObjectOfType<GamesCodeDataSource>();
        registerInitialLoad += game.AwakeDB;

        print("SQLiteDataSource Awake");
        if (string.IsNullOrEmpty(this.databaseName))//se esse nome for null ou vazio
        {
            Debug.LogError("Database name is empty!!!!"); // sabe q falta informar o nome do bd
            return;
        }

        if (this.copyDatabase)
        {
            CopyDatabaseFileIfNotExists();
        }
        else
        {
            //CreateDatabaseFileIfNotExists();
        }

        var savedVersion = PlayerPrefs.GetInt(tagVersion, -1);
        if(codeVersion > savedVersion)//verifica se precisa migrar
        {
            if(savedVersion == -1)//se rodar pela primeira vez
            {
                savedVersion = codeVersion;
            }

            for(var v = savedVersion + 1; v <= codeVersion; v++)
            {
                using(var connection = Connection)
                {
                    connection.Open();
                    using(var command = connection.CreateCommand())
                    {
                        try
                        {
                            var scripts = SQLMigration.versionScripts[v];
                            for(var s = 0; s <scripts.Count; s++)
                            {
                                command.CommandText = scripts[s];
                                command.ExecuteNonQuery();
                            }
                        }
                        catch //se nao tiver nenhuma chave
                        {
                        }
                    }
                }
            }

            PlayerPrefs.SetInt(tagVersion, codeVersion);
        }

    }

    //recurso para organizar
    #region Create Database 
    private void CopyDatabaseFileIfNotExists()// apenas copiara o arquivo se ele nao existe
    {
        this.databasePath = Path.Combine(Application.persistentDataPath, this.databaseName);

        if (File.Exists(this.databasePath))//se existe nao precisa fazer nada
        {
            registerInitialLoad?.Invoke();
            return;
        }
        var isAndroid = false; // se ja foi copiada pelo android
        var originalDatabasePath = string.Empty;//determinar a origem do arquivo, inicia vazia, 

        //windownPhone, aplicativo uwp
#if UNITY_EDITOR || UNITY_WP8 || UNITY_WINRT || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX

        originalDatabasePath = Path.Combine(Application.streamingAssetsPath, this.databaseName);

#elif UNITY_STANDALONE_OSX
 
        originalDatabasePath = Path.Combine(Application.dataPath, "/Resources/Data/StreamingAssets/", this.DatabaseName);

#elif UNITY_IOS
        
       originalDatabasePath = Path.Combine(Application.dataPath, "Raw", this.DatabaseName);

#elif UNITY_ANDROID

       isAndroid = true;
       originalDatabasePath =  "jar:file://" + Application.dataPath + "!/assets/" + this.DatabaseName;
       StartCoroutine(GetInternalFileAndroid(originalDatabasePath));

#endif

        if (!isAndroid)
        {
            File.Copy(originalDatabasePath, this.databasePath);
            registerInitialLoad?.Invoke();
        }
    }

    private void CreateDatabaseFileIfNotExists()//criar o bd se ele nao existir ainda
    {
        //combinar diretorio , caminho ao nosso arquivo de bd
        this.databasePath = Path.Combine(Application.persistentDataPath, this.databaseName);

        if (!File.Exists(this.databasePath))//se nao existe nesse diretotio criar
        {
            SqliteConnection.CreateFile(this.databasePath);//criar somente o arquivo
            Debug.Log($"Database Path: {this.databasePath}");
        }
    }

    private IEnumerator GetInternalFileAndroid(string path)
    {
        var request = UnityWebRequest.Get(path);//leitura como se fosse http
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)//se tiver erro de leitura no arquivo
        {
            Debug.LogError($"Error reading android file: {request.error}");
        }
        else // vai receber um pacote de byts(copia)
        {                       //onde vai , o que vai receber
            File.WriteAllBytes(this.databasePath, request.downloadHandler.data);
            Debug.Log("File copied!");

            //if (registerInitialLoad != null)
            registerInitialLoad?.Invoke();
        }
    }
    #endregion
}
