using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = Inventor.Application;

namespace Sprocket
{
    /// <summary>
    /// Класс менеджер для запуска/перехвата приложения Inventor Autodesk
    /// </summary>
    internal class InventorManager
    {
        /// <summary>
        /// Свойство для хранения ссылки на приложение 
        /// </summary>
        public Application InvApp { get; set; }

        /// <summary>
        /// Конструктор с инициализацией приложения.
        /// </summary>
        public InventorManager()
        {
            // Инициализируем поле.
            InvApp = null;
            try
            {
                //Пытаемся перехватить контроль над приложением инвентора, и сделать его видимым.
                InvApp = (Application)Marshal.GetActiveObject("Inventor.Application");
            }
            catch (Exception)
            {
                try
                {
                    //Если не получилось перехватить приложение - выкинется сообщение на то,
                    //что такого активного приложения нет. Попробуем создать приложение вручную.
                    Type invAppType = Type.GetTypeFromProgID("Inventor.Application");
                    InvApp = (Application)Activator.CreateInstance(invAppType);
                    InvApp.Visible = true;
                }
                catch (Exception ex)
                {
                    //Если ничего не получилось - выведем сообщение о том, что инвентор не установлен,
                    //либо по каким-то причинам не получилось до него добраться.
                    MessageBox.Show(ex.ToString());
                    MessageBox.Show(@"Не удалось запустить или подключиться к Inventor");
                }
            }

        }
    }
}
