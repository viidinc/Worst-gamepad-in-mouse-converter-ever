using SharpDX.DirectInput;
using System.Windows.Forms;
using WindowsInput;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Diagnostics;
class Program
{

    static void Main(string[] args)
    {

        Vector2 Center = new Vector2(32768, 32768);
        Vector2 DeadZone = new Vector2(10000, 10000);
        Vector2 Delta = new Vector2(0, 0);
        Vector2 mouseButtons = new Vector2(0, 0);
        int delay = 100;
        int scrollDelay = 100;

        JoystickState state;
        Joystick gamepad;

        InputSimulator cis = new InputSimulator();
        DirectInput directInput = new DirectInput(); // Инициализация DirectInput
        DeviceInstance gamepadInstance = null; // Переменная для хранения экземпляра геймпада

    // Поиск подключенных устройств
    MainLoop:
        foreach (var device in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
        {
            gamepadInstance = device; // Если найдено устройство, сохраняем его
            if (gamepadInstance != null) { //Console.WriteLine("gameInstance find");
                                         }
            break; // Выходим из цикла, если нашли хотя бы один геймпад
        }

        // Проверка, найден ли геймпад
        if (gamepadInstance == null)
        {
            //Console.WriteLine("gn");
            goto MainLoop;
        }

        try
        {
            // Создание объекта для работы с геймпадом
            gamepad = new Joystick(directInput, gamepadInstance.InstanceGuid);
            gamepad.Acquire(); // Получаем доступ к геймпаду

            //Console.WriteLine("Геймпад подключен. Нажмите 'Esc' для выхода.");

            // Основной цикл программы
            while (true)
            {
                // Обработка события нажатия клавиш
                if (delay > 0) { delay -= 1; }
                if (scrollDelay > 0) { scrollDelay -= 1; }

                gamepad.Poll(); // Обновляем состояние геймпада
                state = gamepad.GetCurrentState(); // Считываем текущее состояние




                if (state.Buttons[0] == false && mouseButtons.X == 1)
                {
                    cis.Mouse.LeftButtonUp();
                    mouseButtons.X = 0;
                }
                if (state.Buttons[1] == false && mouseButtons.Y == 1)
                {
                    cis.Mouse.RightButtonUp();
                    mouseButtons.Y = 0;
                }





                // Вывод состояния кнопок
                for (int i = 0; i < state.Buttons.Length; i++)
                {

                    if (state.Buttons[i] && delay <= 0)
                    {
                       // Console.WriteLine(i);

                        delay += 250000;
                        switch (i)
                        {
                            case (0):
                                cis.Mouse.LeftButtonDown();
                                mouseButtons.X = 1;
                                //Console.WriteLine(0);
                                break;
                            case (1):
                                cis.Mouse.RightButtonDown();
                                //Console.WriteLine(1);
                                mouseButtons.Y = 1;
                                break;
                            case (2):
                                cis.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
                                break;
                            case (7):
                                cis.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                                break;
                        }
                    }
                }


                Delta = Vector2.Zero;

                if (
                    Math.Abs(state.X - Center.X) > DeadZone.X |
                    Math.Abs(state.Y - Center.Y) > DeadZone.Y
                    )
                {
                    

                    Delta.X = (state.X - Center.X);
                    Delta.Y = (state.Y - Center.Y);
                    Delta.X *= (float)0.0005;
                    Delta.Y *= (float)0.0005;
                    Delta.X = Math.Clamp(Delta.X, -100, 100);
                    Delta.Y = Math.Clamp(Delta.Y, -100, 100);
                    cis.Mouse.MoveMouseBy((int)Delta.X, (int)Delta.Y);
                    System.Threading.Thread.Sleep(10);
                }
                
                if (
                    (Math.Abs(state.RotationX - Center.X) > DeadZone.X |
                    Math.Abs(state.RotationY - Center.Y) > DeadZone.Y ) &&
                    scrollDelay <= 0
                    )
                {
                    scrollDelay += 100000;
                    cis.Mouse.VerticalScroll(-Math.Sign(state.RotationY - Center.Y));


                }




                // Проверка, нажата ли клавиша 'Esc' для выхода
                    /*if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        break; // Выход из цикла, если 'Esc' нажата
                    }
                    */

            }


            gamepad.Unacquire(); // Освобождаем геймпад
            gamepad.Dispose(); // Освобождаем ресурсы
            directInput.Dispose();
        } // Освобождаем ресурсы DirectInput
        catch { goto MainLoop; }
        
    
        
    }
}