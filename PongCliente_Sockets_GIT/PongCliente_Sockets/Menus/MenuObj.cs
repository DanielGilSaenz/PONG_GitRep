using System;

namespace PongCliente_Sockets
{
    [Serializable]
    class MenuObj
    {
        public string[] Options { get; private set; } = { "Jugar", "Configuracion", "Salir" };
        public string[] InputFields { get; set; }

        public bool hasBack { get; set; }

        public int selectedOption { get; set; }

        public int BACK { get; set; } = -1;
        public int EXIT { get; set; } = -1;

        public MenuObj()
        {
            selectedOption = 0;
            if(hasBack) BACK = Options.Length - 2;
            EXIT = Options.Length - 1;
        }

        public MenuObj(string[] options, string[] inputFields, bool hasBack)
        {
            this.Options = options;
            this.InputFields = inputFields;
            this.hasBack = hasBack;

            selectedOption = 0;
            if (hasBack) BACK = options.Length - 2;
            EXIT = options.Length - 1;
        }

        public void optionUp()
        {
            if (selectedOption == 0) selectedOption = selectedOption;
            else selectedOption--;
        }

        public void optionDown()
        {
            if (selectedOption == Options.Length - 1) selectedOption = selectedOption;
            else selectedOption++;
        }
    }
}
