namespace Server;

using NLog;
using Grpc.Core;

//this comes from GRPC generated code
using Services;


/// <summary>
/// Service. This is made to run as a singleton instance.
/// </summary>
public class Service : Services.Service.ServiceBase
{
    /// <summary>
    /// Logger for this class.
    /// </summary>
    Logger log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Access lock.
    /// </summary>
    private AdapterLogic logic = new AdapterLogic();

	/// <summary>
	/// Get hour according to the 24h system from the server. Is used by eater and baker to determine if it is time to bake or eat.
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>Unique ID.</returns>
	public override Task<IntMsg> GetHour(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var result = new IntMsg { Value = logic.GetHour()};
            return Task.FromResult(result);
        }
	}

	/// <summary>
	/// Determines if it is time for the baker to bake or not
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>True on time to bake, false on not time to bake.</returns>
	public override Task<BoolMsg> BakerBakes(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var result = new BoolMsg { Value = logic.BakerBakes()};
            return Task.FromResult(result);
        }
	}

	/// <summary>
	/// Determines if it is time for the eater to eat or not
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>True on time to eat, false on not time to eat.</returns>
	public override Task<BoolMsg> EaterEats(Empty input, ServerCallContext context)
	{ 
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var result = new BoolMsg { Value = logic.EaterEats()};
            return Task.FromResult(result);
        }
	}

	/// <summary>
	/// Determines if the canteen is closed or not
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>True on canteen is closed, false on canteen is open.</returns>
	public override Task<BoolMsg> CloseCanteen(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var result = new BoolMsg { Value = logic.CloseCanteen()};
            return Task.FromResult(result);
        }
	}

	/// <summary>
	/// Start the countdown for a day (24hours)
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>Unique ID.</returns>
	public override Task<Empty> Count24H(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            logic.CloseCanteen();
            return Task.FromResult(new Empty());
        }
	}

	/// <summary>
	/// Baker bakes new portions of food
	/// </summary>
	/// <param name="input">number of portions to bake.</param>
	/// <param name="context">Call context.</param>
	public override Task<Empty> Bake(IntMsg input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var intValue = input.Value; 
            logic.Bake(intValue);
            return Task.FromResult(new Empty());
        }
	}

	/// <summary>
	/// Eater eat portions of food
	/// </summary>
	/// <param name="input">number of portions to bake.</param>
	/// <param name="context">Call context.</param>
	/// <returns>Unique ID.</returns>
	public override Task<Empty> Eat(IntMsg input, ServerCallContext context)
	{ 
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            var intValue = input.Value; 
            logic.Eat(intValue);
            return Task.FromResult(new Empty());
        }
		
	}

	/// <summary>
	/// Baker leaves the canten
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>Unique ID.</returns>
	public override Task<Empty> BakerLeaves(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            logic.BakerLeaves();
            return Task.FromResult(new Empty());
        }
	}

	/// <summary>
	/// Eater leaves the canten
	/// </summary>
	/// <param name="input">Not used.</param>
	/// <param name="context">Call context.</param>
	/// <returns>Unique ID.</returns>
	public override Task<Empty> EaterLeaves(Empty input, ServerCallContext context)
	{
        log.Info($"Service instance hash code: {this.GetHashCode()}.");

        lock(logic){
            logic.EaterLeaves();
            return Task.FromResult(new Empty());
        }
	}


}