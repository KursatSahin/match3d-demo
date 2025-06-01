using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using Timer = Match3d.Core.Utils.Timer;

namespace Match3d.Gameplay.Boosters
{
    public class TimeFreeze : IBooster
    {
        private const int Duration = 15000;
        
        [Inject] private Timer _levelTimer;
        
        public string Name { get; }
        
        public BoosterData.BoosterType Type => BoosterData.BoosterType.TimeFreeze;
        public async UniTask Execute(CancellationToken token)
        {
            _levelTimer.IsRunning = false;

            await UniTask.Delay(Duration, cancellationToken: token).SuppressCancellationThrow();

            _levelTimer.IsRunning = true;
        }

        public void Complete()
        {
        }
    }
}