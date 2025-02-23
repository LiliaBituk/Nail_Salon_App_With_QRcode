using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nail_Salon_Mobile_App_New
{
    public partial class Design
    {
        public static void ApplyTheme(ResourceDictionary resources)
        {
            if (Application.Current.RequestedTheme == AppTheme.Dark)
            {
                resources["BackgroundColorLight"] = Color.FromArgb("#3C0A1D"); // Темно-розовый фон
                resources["TextColorLight"] = Color.FromArgb("#FF6F91"); // Темно-розовый текст
                resources["EntryBackgroundLight"] = Color.FromArgb("#4A1A2D"); // Темный фон для Entry
                resources["EntryTextColorLight"] = Colors.White; // Белый текст для Entry
                resources["ButtonBackgroundLight"] = Color.FromArgb("#FF6F91"); // Темно-розовая кнопка
                resources["ButtonTextColorLight"] = Colors.Black; // Черный текст для кнопки
            }
            else
            {
                resources["BackgroundColorLight"] = Color.FromArgb("#FFF0F6"); // Светло-розовый фон
                resources["TextColorLight"] = Color.FromArgb("#D81B60"); // Светло-розовый текст
                resources["EntryBackgroundLight"] = Colors.White; // Белый фон для Entry
                resources["EntryTextColorLight"] = Colors.Black; // Черный текст для Entry
                resources["ButtonBackgroundLight"] = Color.FromArgb("#D81B60"); // Светло-розовая кнопка
                resources["ButtonTextColorLight"] = Colors.White; // Белый текст для кнопки
            }
        }
    }
}
