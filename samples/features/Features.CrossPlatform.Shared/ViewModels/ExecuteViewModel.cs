using System.Threading.Tasks;
using Caliburn.Micro;
using Features.CrossPlatform.Views;
using System;
using System.Diagnostics;

namespace Features.CrossPlatform.ViewModels
{
    public class ExecuteViewModel : Screen
    {
        private bool _safe;

        public bool Safe
        {
            get { return _safe; }
            set { Set(ref _safe, value); }
        }

        public void StartBackgroundWork()
        {
            Task.Factory.StartNew(BackgroundWork);
        }

        private void BackgroundWork()
        {
            if (Safe)
                SafeBackgroundWork();
            else
                UnsafeBackgroundWork();
        }

        private void SafeBackgroundWork()
        {
            Execute.OnUIThreadAsync(UpdateView);
        }

        private void UnsafeBackgroundWork()
        {
            try
            {
                UpdateView();
            }
            catch (Exception ex)
            {
                //Execute.OnUIThreadAsync(() =>
                //{
                //    Debug.WriteLine("Exception: " + ex.Message);
                //    return Task.CompletedTask;
                //});

                throw;
            }
        }

        private Task UpdateView()
        {
            var view = (ExecuteView)GetView();

            view.UpdateView();

            return Task.CompletedTask;
        }
    }
}
