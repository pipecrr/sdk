using Microsoft.AspNetCore.Components;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Protos;
using Blazored.Toast.Services;
using Siesa.SDK.Shared.Services;
using Blazored.Toast.Configuration;
using Blazored.Toast;


namespace Siesa.SDK.Frontend.Components.Layout
{
    /// <summary>
    /// Componente para mostrar notificaciones
    /// </summary>
    public partial class NotificationsComponent : SDKComponent, IDisposable
    {

        [Inject] private IQueueService QueueService { get; set; }
        [Inject] private IToastService ToastService { get; set; }

        private List<Siesa.SDK.Protos.QueueMessageDTO> Notifications { get; set; }

        private List<Siesa.SDK.Protos.QueueMessageDTO> TempsNotifications { get; set; }

        private string Message { get; set; }

        private bool _OpenNotifications = false;
        private bool FloatingNotification = false;

         private ToastParameters _toastParameters = new ToastParameters();

        protected override async Task OnInitializedAsync()
        {
            QueueService.Subscribe("BLUser", enumMessageCategory.CRUD, this.OnCrudNotification);

            await base.OnInitializedAsync();
        }

        public void OnCrudNotification(QueueMessageDTO message)
        {
            Console.WriteLine($"Desde Notification Component {message.Message}");

            if (Notifications == null)
                Notifications = new List<Siesa.SDK.Protos.QueueMessageDTO>();

            if (message != null)
                Notifications.Add(message);

            _toastParameters.Add(nameof(DemoToast.Title), "Ivan Grisales");
            _toastParameters.Add(nameof(DemoToast.IconName), "fa-code");
            _toastParameters.Add(nameof(DemoToast.Message), message.Message);
            _toastParameters.Add(nameof(DemoToast.Category), message.QueueName);


            ToastService.ShowToast<DemoToast>(_toastParameters,settings => { 
                settings.Timeout = 3;});

            InvokeAsync(() => StateHasChanged());

        }

        public void Dispose()
        {
            QueueService.Unsubscribe("BLUser", enumMessageCategory.CRUD);
        }

        private void OpenNotification()
        {
            _OpenNotifications = !_OpenNotifications;
            StateHasChanged();
        }

        private void RemoveNotification(Siesa.SDK.Protos.QueueMessageDTO notify)
        {
            Notifications.Remove(notify);
            StateHasChanged();
        }


    }

}