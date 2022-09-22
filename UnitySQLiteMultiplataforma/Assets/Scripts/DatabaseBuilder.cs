using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseBuilder : MonoBehaviour
{
    public string DatabaseName; // nome do arquivo
    protected string databasePath;//caminho para o bd              
    protected SqliteConnection Connection => new SqliteConnection($"Data source = {this.databasePath};");

    private void Awake()
    {
        if (string.IsNullOrEmpty(this.DatabaseName))//se esse nome for null ou vazio
        {
            Debug.LogError("Database name is empty!!!!"); // sabe q falta informar o nome do bd
            return;
        }
        CreateDatabaseFileIfNotExists();
    }

    private void CreateDatabaseFileIfNotExists()//criar o bd se ele nao exitir ainda
    {
        //combinar diretorio , caminho ao nosso arquivo de bd
        this.databasePath = Path.Combine(Application.persistentDataPath, this.DatabaseName);

        if (!File.Exists(this.databasePath))//se nao existe nesse diretotio criar
        {
            SqliteConnection.CreateFile(this.databasePath);//criar somente o arquivo
            Debug.Log($"Database Path: {this.databasePath}");
        }
    }
}
