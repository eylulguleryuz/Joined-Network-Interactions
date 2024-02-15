namespace Servers;

using Services;


/// <summary>
/// Service
/// </summary>
public class Service : ICanteenService
{
    /// <summary>
    /// Access lock.
    /// </summary>
    private readonly Object accessLock = new Object();

    /// <summary>
    /// Service logic implementation.
    /// </summary>
    private AdapterLogic logic = new AdapterLogic();

   
	/// <summary>
	/// Countdown for a day (24hours)
	/// </summary>
    public void Count24H()
    {
        lock (accessLock)
        {
            logic.Count24H();
        }
    }

     /// <summary>
    /// Baker bakes new portions of food
    /// </summary>
    /// <param name="baked">amount of portions to bake</param>
	public void Bake(int baked){
        lock (accessLock){
		    logic.Bake(baked);
        }
    }

     /// <summary>
    /// Eater eats indicated amount of portions and necessary actions are implemented 
    /// </summary>
    /// <param name="eat">portions to eat</param>
	public void Eat(int eat){
        lock(accessLock)
        {
		    logic.Eat(eat);
        }
    }

    /// <summary>
    /// Baking time
    /// </summary>
    /// <returns>if it is time or not</returns>
	public bool BakerBakes(){
        lock(accessLock)
        {
		    return logic.BakerBakes();
        }
	}

    /// <summary>
    /// Eating time
    /// </summary>
    /// <returns>If it is time or not</returns>
	public bool EaterEats(){
        lock(accessLock){
            return logic.EaterEats();
        }
	}

    /// <summary>
    /// Checks if the canteen is closed
    /// </summary>
    /// <returns>if canteen is closed or not</returns>
    public bool CloseCanteen(){
        lock(accessLock){
            return logic.CloseCanteen();
        }
    }

    /// <summary>
    /// Logs baker state (leaving)
    /// </summary>
    public void BakerLeaves(){
      lock(accessLock){
        logic.BakerLeaves();
      }
    }

    /// <summary>
    /// Logs eater state (leaving)
    /// </summary>
    public void EaterLeaves(){
      lock(accessLock){
        logic.EaterLeaves();
      }
    }

	/// <summary>
    /// Gets the time according to the 24hr system
    /// </summary>
    /// <returns>the current hour</returns>
    public int GetHour(){
        lock(accessLock){
            return logic.GetHour();
        }
    }
}
