using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace D1.Data
{
    public static class InputValidation
    {

        public static bool ValidateDigit(KeyEventArgs e)
        {

            char character = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            if (!char.IsDigit(character) && e.Key != Key.Back)
                return true;


            return false;

        }


        public static bool ValidatePrice(KeyEventArgs e)
        {

            char character = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            if (!char.IsDigit(character) && e.Key != Key.Back && character != 188)
                return true;


            return false;

        }






    }
}
