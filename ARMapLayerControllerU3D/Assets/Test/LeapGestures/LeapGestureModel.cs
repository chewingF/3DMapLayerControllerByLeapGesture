//Harry Chen 2018/07/14 create for the structure of program to detect gestures by leap motion 
using Leap.Unity;
using Leap;

public class LeapGestureModel {
    public enum State
    {
        Free,
        InProcess,
        End,
    }

    protected State _detectingState;
    public State DetectingState
    {
        get
        {
            return _detectingState;
        }
    }

    protected float ProcessTime, countTime;
    protected Hand leftHand, rightHand;

    public float CountTime
    {
        get
        {
            return countTime;
        }
    }

    public LeapGestureModel()
    {
        _detectingState = State.Free;
        ProcessTime = 1.0f;
        countTime = 0f;
    }

    public State Check(Hand leftHand, Hand rightHand, float deltaTime)
    {
        this.leftHand = leftHand;
        this.rightHand = rightHand;
        switch (DetectingState)
        {
            case State.Free:
                if (checkStart())
                {
                    this._detectingState = State.InProcess;
                }
                return this.DetectingState;
            case State.InProcess:
                //when the gesture is not continuing, set state back to free.
                if (!checkInProcess())
                {
                    this._detectingState = State.Free;
                    this.countTime = 0;
                    return this.DetectingState;
                }
                else
                {
                    //otherwise, count the process time
                    countTime += deltaTime;
                    //check if the gesture is finished
                    if (checkEnd())
                    {
                        this._detectingState = State.End;
                    }
                    return this.DetectingState;
                }
            case State.End:
                gestureDetected();
                this._detectingState = State.Free;
                this.countTime = 0;
                return this.DetectingState;
        }
        return this.DetectingState;
    }
       

    public void setProcessTime(float ProcessTime)
    {
        this.ProcessTime = ProcessTime;
    }

    public State getDetectingState()
    {
        return this.DetectingState;
    }

    //to be override as detecting the start signal of a gusture
    public virtual bool checkStart()
    {
        return false;
    }

    //to be override as detecting if the gesture is still in process
    public virtual bool checkInProcess()
    {
        return false;
    }

    //to be override as detecting if the gesture is finished
    public virtual bool checkEnd()
    {
        return false;
    }

    //to be override as the commend give by the gesture
    public virtual void gestureDetected()
    {
        return;
    }
    
}
