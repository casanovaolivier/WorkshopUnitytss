public enum Gamepad { None, PS4, Xbox, JoyCon, Unknown }
public enum Player { Unknown, One, Two, Three, Four }
public enum GamepadManagerState { DoNothing, WaitForFirstJoystick, CheckForOneJoystick, CheckForTwoJoystick, CheckForThreeJoystick, CheckForFourJoystick, WaitForDisconnection }

public enum PS4_Axis { LeftStickX = 1, LeftStickY = 2, RightStickX = 3, RightStickY = 6, DPadX = 7, DPadY = 8 }
public enum PS4_Button { Square = 0, Cross = 1, Circle = 2, Triangle = 3, L1 = 4, R1 = 5, L2 = 6, R2 = 7, Share = 8, Options = 9, LeftStick = 10, RightStick = 11, PS = 12, Touchpad = 13 }
public enum PS4_ButtonForUI { Square = 0, Cross = 1, Circle = 2, Triangle = 3 }

public enum Xbox_Axis { LeftStickX = 1, LeftStickY = 2, RBToLB = 3, RightStickX = 4, RightStickY = 5, DPadX = 6, DPadY = 7, LT = 9, RT = 10 }
public enum Xbox_Button { A = 0, B = 1, X = 2, Y = 3, LB = 4, RB = 5, Select = 6, Start = 7, LeftStick = 8, RightStick = 9 }
public enum Xbox_ButtonForUI { A = 0, B = 1, X = 2, Y = 3 }

public enum JoyCon_Axis { StickX = 9, StickY = 10 }
public enum JoyCon_Button { Down = 0, Right = 1, Left = 2, Up = 3, SL = 4, SR = 5, Minus = 8, Plus = 9, MinusStick = 10, PlusStick = 11, Home = 12, Capture = 13 }
public enum JoyCon_ButtonForUI { Down = 0, Right = 1, Left = 2, Up = 3 }

public enum JoyGrip_Axis { LeftStickX = 9, LeftStickY = 10, RightStickX = 9, RightStickY = 10 }
public enum JoyGrip_Button { Down = 0, Right = 1, Left = 2, Up = 3, A = 0, X = 1, B = 2, Y = 3, Minus = 8, Plus = 9, LeftStick = 10, RightStick = 11, Home = 12, Capture = 13 }