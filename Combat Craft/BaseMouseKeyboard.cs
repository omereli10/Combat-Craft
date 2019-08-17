using Microsoft.Xna.Framework.Input;

namespace Combat_Craft
{
    abstract class BaseMouseKeyboard
    {
        #region Data

        public int mouseWheel_Value;
        public int previousMouseWheel_Determine;
        public bool isHolding_MouseLeftButton;
        public bool isHolding_MouseRightButton;
        public bool isHolding_KeyboardFlyingMode;
        public bool isHolding_KeyboardMouseLock;
        public bool isHolding_KeyboardScrollUp;
        public bool isHolding_KeyboardScrollDown;
        public MouseState currentMouseState;
        public MouseState previosMouseState;

        #endregion Data

        #region Operations
        public abstract bool IsWalkRight();
        public abstract bool IsWalkLeft();
        public abstract bool IsWalkForward();
        public abstract bool IsWalkBackward();
        public abstract bool IsWalkUp();
        public abstract bool IsWalkDown();
        public abstract bool IsRunRight();
        public abstract bool IsRunLeft();
        public abstract bool IsRunForward();
        public abstract bool IsRunBackward();
        public abstract bool IsRunUp();
        public abstract bool IsRunDown();
        public abstract bool IsSlowdownRight();
        public abstract bool IsSlowdownLeft();
        public abstract bool IsSlowdownForward();
        public abstract bool IsSlowdownBackward();
        public abstract bool IsSlowdownUp();
        public abstract bool IsSlowdownDown();
        public abstract bool IsFlyingMode();
        public abstract bool IsJump();
        public abstract bool IsMouseLock();
        public abstract bool IsScrollUp();
        public abstract bool IsScrollDown();
        #endregion Operations
    }

    class UserKeyboard : BaseMouseKeyboard
    {
        #region Keys

        public Keys Right { get; private set; }
        public Keys Left { get; private set; }
        public Keys Forward { get; private set; }
        public Keys Backward { get; private set; }
        public Keys Up { get; private set; }
        public Keys Down { get; private set; }
        public Keys Run_Boost { get; private set; }
        public Keys Slowdown { get; private set; }
        public Keys FlyingMode { get; private set; }
        public Keys Jump { get; private set; }
        public Keys MouseLock { get; private set; }
        public Keys ScrollUp { get; private set; }
        public Keys ScrollDown { get; private set; }

        #endregion Keys

        #region Constractors

        public UserKeyboard(Keys right, Keys left, Keys forward, Keys backward, Keys up, Keys down, Keys run_boost, Keys slowdown, Keys flyingMode, Keys jump, Keys mouseLock, Keys scrollUp, Keys scrollDown)
        {
            this.Right = right;
            this.Left = left;
            this.Forward = forward;
            this.Backward = backward;
            this.Up = up;
            this.Down = down;
            this.Run_Boost = run_boost;
            this.Slowdown = slowdown;
            this.FlyingMode = flyingMode;
            this.Jump = jump;
            this.MouseLock = mouseLock;
            this.ScrollUp = scrollUp;
            this.ScrollDown = scrollDown;

            this.mouseWheel_Value = 0;
            this.isHolding_MouseLeftButton = false;
            this.isHolding_MouseRightButton = false;
            this.isHolding_KeyboardFlyingMode = false;
        }

        #endregion Constractors

        #region KeysStates

        public override bool IsWalkRight()
        {
            return Keyboard.GetState().IsKeyDown(Right) && !(Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown)));
        }
        public override bool IsWalkLeft()
        {
            return Keyboard.GetState().IsKeyDown(Left) && !(Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsWalkForward()
        {
            return Keyboard.GetState().IsKeyDown(Forward) && !Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsWalkBackward()
        {
            return Keyboard.GetState().IsKeyDown(Backward) && !(Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsWalkUp()
        {
            return Keyboard.GetState().IsKeyDown(Up) && !(Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsWalkDown()
        {
            return Keyboard.GetState().IsKeyDown(Down) && !(Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunRight()
        {
            return Keyboard.GetState().IsKeyDown(Right) && Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunLeft()
        {
            return Keyboard.GetState().IsKeyDown(Left) && Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunForward()
        {
            return Keyboard.GetState().IsKeyDown(Forward) && (Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunBackward()
        {
            return Keyboard.GetState().IsKeyDown(Backward) && (Keyboard.GetState().IsKeyDown(Run_Boost)) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunUp()
        {
            return Keyboard.GetState().IsKeyDown(Up) && Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsRunDown()
        {
            return Keyboard.GetState().IsKeyDown(Down) && Keyboard.GetState().IsKeyDown(Run_Boost) && !(Keyboard.GetState().IsKeyDown(Slowdown));
        }
        public override bool IsSlowdownRight()
        {
            return Keyboard.GetState().IsKeyDown(Right) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsSlowdownLeft()
        {
            return Keyboard.GetState().IsKeyDown(Left) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsSlowdownForward()
        {
            return Keyboard.GetState().IsKeyDown(Forward) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsSlowdownBackward()
        {
            return Keyboard.GetState().IsKeyDown(Backward) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsSlowdownUp()
        {
            return Keyboard.GetState().IsKeyDown(Up) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsSlowdownDown()
        {
            return Keyboard.GetState().IsKeyDown(Down) && Keyboard.GetState().IsKeyDown(Slowdown) && !Keyboard.GetState().IsKeyDown(Run_Boost);
        }
        public override bool IsFlyingMode()
        {
            return Keyboard.GetState().IsKeyDown(FlyingMode);
        }
        public override bool IsJump()
        {
            return Keyboard.GetState().IsKeyDown(Jump);
        }
        public override bool IsMouseLock()
        {
            return Keyboard.GetState().IsKeyDown(MouseLock);
        }
        public override bool IsScrollUp()
        {
            return Keyboard.GetState().IsKeyDown(ScrollUp);
        }
        public override bool IsScrollDown()
        {
            return Keyboard.GetState().IsKeyDown(ScrollDown);
        }

        #endregion KeysStates
    }
}