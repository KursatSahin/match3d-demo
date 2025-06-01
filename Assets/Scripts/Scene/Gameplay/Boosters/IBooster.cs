using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3d.Gameplay.Boosters
{
    public interface IBooster
    {
        public string Name { get; }
        
        public BoosterData.BoosterType Type { get; }

        public UniTask Execute(CancellationToken token);
        public void Complete();
    }
}