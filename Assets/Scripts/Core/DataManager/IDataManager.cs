using System.Collections.Generic;

namespace Match3d.Core.DataManager
{
    public interface IDataManager
    {
        public void Save(PlayerData data);
        public PlayerData Load();
        public void Reset();
    }
}