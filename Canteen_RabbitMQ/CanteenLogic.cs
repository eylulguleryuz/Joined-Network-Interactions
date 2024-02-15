namespace Servers;

using NLog;

using Timer = System.Timers.Timer;

using Services;

/// <summary>
/// CanteenState descriptor.
/// </summary>
public class CanteenState
{
	/// <summary>
	/// Access lock.
	/// </summary>
	public readonly object AccessLock = new object();


	/// <summary>
	/// Canteen state.
	/// </summary>
	public CanteenState State;

}

public class CanteenLogic
{
	int hour = 1;
    int foodStored = 0;
    int closedDays = 0;  
    int profit = 0;
    double reputation = 0;
    bool closedOrNot = false;
    int negativeDays = 0;
    int day = 1;

	/// <summary>
	/// Logger for this class.
	/// </summary>
	private Logger mLog = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// State descriptor.
	/// </summary>
	private CanteenState mState = new CanteenState();
	
	/// <summary>
	/// Constructor.
	/// </summary>
	public CanteenLogic()
	{
		Count24H();
	}
    /// <summary>
    /// Baker bakes new portions of food
    /// </summary>
    /// <param name="baked">amount of portions to bake</param>
	public void Bake(int baked){
        lock (mState.AccessLock){
            
            if(BakerBakes()){
                    foodStored += baked;
                    mLog.Info($"Baker baked {baked} portions at {hour}");
            }   
        }
    }
    /// <summary>
    /// Baking time
    /// </summary>
    /// <returns>if it is time or not</returns>
	public bool BakerBakes(){
        lock (mState.AccessLock){
            return (hour > 7 && hour < 17);
        }
    }
    /// <summary>
    /// Eating time
    /// </summary>
    /// <returns>If it is time or not</returns>
	public bool EaterEats(){
        lock (mState.AccessLock){
            return (hour > 10 && hour < 19);
        }
    }
    /// <summary>
    /// Logs baker state
    /// </summary>
	 public void BakerLeaves(){
       mLog.Info($"Baker is not working.");
    }
    /// <summary>
    /// Logs eater state
    /// </summary>
	public void EaterLeaves(){
        mLog.Info($"Eater is not in the canteen.");
    }
    /// <summary>
    /// All the food in the storage gets discarded
    /// </summary>
     private void DiscardFood(){
        lock (mState.AccessLock)
        {
            DecreaseProfit(foodStored);
            foodStored = 0;
        }
    }
    /// <summary>
    /// Eater eats indicated amount of portions and necessary actions are implemented 
    /// </summary>
    /// <param name="eat">portions to eat</param>
    public void Eat(int eat){
        lock (mState.AccessLock){
            if (EaterEats())
            {
                if(foodStored >= eat){
                        foodStored -= eat;

                        mLog.Info($"Eater ate {eat} portions of food at {hour}");
                        IncreaseProfit(eat);
                        IncreaseReputation(eat);
                        mLog.Info($"In the storage : {foodStored} portions left ");
                        mLog.Info("+++++Reputation and profit are up.");
                    
                }else{
                    DecreaseReputation(eat-foodStored);
                    mLog.Info($"Eater wanted {eat} portions of food at {hour}");
                    mLog.Info($"In the storage : There are {foodStored} portions ");
                    mLog.Info($"-----There isn't enough food. Reputation down.");
                    foodStored = 0;
                }
            }
        }
    }
    /// <summary>
    /// Countdown for a day (24hours)
    /// </summary>
    /// <param name="interval">every hour is 2 seconds</param>
  	public void Count24H(int interval = 2000){

        var timer = new Timer(interval){
            AutoReset = true
        };
        timer.Elapsed += (_,_) =>
        {
            DayGoesOn();
        };
        timer.Enabled = true;
    }
    /// <summary>
    /// Day activities
    /// </summary>
	private void DayGoesOn(){
        lock (mState.AccessLock){
            if (hour == 19 && !closedOrNot){
                mLog.Info($"Food thrownaway at the end of the day: {foodStored} portions");
                DiscardFood();
            }

            if (hour == 24){ 
                mLog.Info("-------------------------------------------------");
                mLog.Info($"END OF DAY {day} " + 
                         $" | PROFIT: {profit} | REPUTATION: {Math.Round(reputation,2)}");
                mLog.Info("-------------------------------------------------");

                if(profit < 0 || reputation < 0){
                    negativeDays++;
                    if(negativeDays == 2){
                        closedOrNot = true;
                        ResetCanteen();
                        mLog.Info("Bad profit and/or reputation!! Canteen is closed down.");
                        mLog.Info("-------------------------------------------------");
                        negativeDays = 0;
                    }
                }else{
                    negativeDays = 0;
                }

                if(closedDays < 1 && closedOrNot){
                    closedDays++;
                }else{
                    closedDays = 0;
                    closedOrNot = false;
                }

                hour = 1;
                day++;
                mLog.Info($"START OF DAY {day}");
            }
            else{
                hour++; 
            }
        }
    }
    /// <summary>
    /// Resets the canteen to a fresh state
    /// </summary>
   private void ResetCanteen(){
            foodStored = 0;
            profit = 0;
            reputation = 0;
    }

    /// <summary>
    /// Increases profit
    /// </summary>
    /// <param name="food">amount of food portions to increase the profit by</param>
    private void IncreaseProfit(int food){
        //one unit of food is 3 euros
        profit += (int)Math.Ceiling((double)food*3.0);
    }
    /// <summary>
    /// Decreases profit
    /// </summary>
    /// <param name="food">amount of food portions to decrease the profit by</param>
    private void DecreaseProfit(int food){
         profit -= (int)Math.Ceiling((double)food);
    }
    /// <summary>
    /// Increases reputation
    /// </summary>
    /// <param name="food">amount of food portions to increase the reputation by</param>
    private void IncreaseReputation(int eaten){
        //reputation rank
        int rank = (int)Math.Ceiling((double)eaten/5.0);
        reputation += (eaten*rank);
    }
    /// <summary>
    /// Decreases reputation
    /// </summary>
    /// <param name="food">amount of food portions to decrease the reputation by</param>
    private void DecreaseReputation(int eaten){
        int rank = (int)Math.Ceiling((double)eaten/5.0);
        reputation -= (eaten*rank);
    }
    /// <summary>
    /// Checks if the canteen is closed
    /// </summary>
    /// <returns>if canteen is closed or not</returns>
	public bool CloseCanteen(){
        lock (mState.AccessLock){
            return closedOrNot;
        }
    }
    /// <summary>
    /// Gets the time according to the 24hr system
    /// </summary>
    /// <returns>the current hour</returns>
     public int GetHour(){ 
      lock (mState.AccessLock){
        return hour;
      }
    }

}