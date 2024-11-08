using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEngine;

public abstract class InterplanetaryAction : MonoBehaviour
{


    // protected IMainFrame mf = null;
    // protected GameMainScreen gms = null;
    protected MainPlanet srcPlanet = null;
    // protected Vector2 srcPoint = null;
    protected MainPlanet dstPlanet = null;
    // protected Vector2 dstPoint = null;
    protected int actionLength = 5000; //5000ms = 5s
    // protected TickEvent firstTick = null;
    protected float actionStartTime = Time.time;
    // protected float actionProgress = 0.0f;
    private bool finished = false;
    protected EPlayerType actionOwner = EPlayerType.NONE;
    private float distance = 0;
    private bool isPaused = false;
    // private long pauseLength = 0, lastPauseStartTime = 0;


    // /**
    //  * 
    //  * @param mf
    //  * @param gms
    //  * @param speed in ms
    //  */
    public InterplanetaryAction(MainPlanet srcPlanet, MainPlanet dstPlanet, int speed): base()
    {
        this.srcPlanet = srcPlanet;
        this.dstPlanet = dstPlanet;
        this.actionOwner = srcPlanet.PlanetOwner;
        // srcPoint = new Vector2(srcPlanet.getXCenter(), srcPlanet.getYCenter());
        // dstPoint = new Vector2(dstPlanet.getXCenter(), dstPlanet.getYCenter());
        distance = Vector3.Distance(srcPlanet.transform.position, dstPlanet.transform.position);
        this.actionLength = (int)(distance / speed) * 100;  //obliczamy ile czasu zajmie droga...
    }

    public abstract void Update();

    // public abstract void finishAction();

    // protected void progressAction(long currentTime)
    // {
    //     if (actionStartTime >= 0)
    //     {
    //         long r = currentTime - actionStartTime - pauseLength;
    //         actionProgress = (float)r / actionLength;
    //     }
    //     if (actionProgress >= 1.0f)
    //     {
    //         finished = true;
    //         finishAction();
    //     }
    // }

    // public void tick(TickEvent e)
    // {
    //     if (actionStartTime < 0)
    //     {
    //         actionStartTime = Calendar.getInstance().getTimeInMillis();
    //     }
    //     progressAction(Calendar.getInstance().getTimeInMillis());
    // }

    // public void disposeTickListener()
    // {

    // }

    // public boolean isFinished()
    // {
    //     return finished;
    // }

    // public Owner getActionOwner()
    // {
    //     return actionOwner;
    // }

    // public Planet getDstPlanet()
    // {
    //     return dstPlanet;
    // }

    // public Planet getSrcPlanet()
    // {
    //     return srcPlanet;
    // }

    // public void setPaused(boolean isPaused)
    // {
    //     if (isPaused)
    //     {
    //         lastPauseStartTime = Calendar.getInstance().getTimeInMillis();
    //     }
    //     else
    //     {
    //         pauseLength += Calendar.getInstance().getTimeInMillis() - lastPauseStartTime;
    //     }
    //     this.isPaused = isPaused;
    // }
}