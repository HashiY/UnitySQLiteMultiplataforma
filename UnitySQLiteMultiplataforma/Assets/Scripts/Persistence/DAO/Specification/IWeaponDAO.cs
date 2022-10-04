using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Persistence.DAO.Specification
{
    public interface IWeaponDAO
    {
        ISQLiteConnectionProvider ConnectionProvider { get; }
        bool SetWeapon(Weapon weapon);
        bool UpdateWeapon(Weapon weapon);
        bool DeleteWeapon(int id);
        Weapon GetWeapon(int id);
    }
}
