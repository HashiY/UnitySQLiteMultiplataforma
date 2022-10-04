using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Persistence.DAO.Specification
{
    public interface ICharacterDAO
    {
        ISQLiteConnectionProvider ConnectionProvider { get; }
        bool SetCharacter(Character character);
        bool UpdateCharacter(Character character);
        bool DeleteCharacter(int id);
        Character GetCharacter(int id);
    }
}
