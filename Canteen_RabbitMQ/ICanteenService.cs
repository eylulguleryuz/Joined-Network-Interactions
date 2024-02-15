namespace Server;

/// <summary>
/// Service contract.
/// </summary>
public interface ICanteenService
{	/// <summary>
	/// Countdown for a day (24hours)
	/// </summary>
	void Count24H();

   /// <summary>
    /// Baker bakes new portions of food
    /// </summary>
    /// <param name="baked">amount of portions to bake</param>
	void Bake(int baked);

	/// <summary>
    /// Eater eats indicated amount of portions and necessary actions are implemented 
    /// </summary>
    /// <param name="eat">portions to eat</param>
	void Eat(int eat);

	/// <summary>
    /// Baking time
    /// </summary>
    /// <returns>if it is time or not</returns>
	bool BakerBakes();

	/// <summary>
    /// Eating time
    /// </summary>
    /// <returns>If it is time or not</returns>
	bool EaterEats();

	/// <summary>
    /// Checks if the canteen is closed
    /// </summary>
    /// <returns>if canteen is closed or not</returns>
	bool CloseCanteen();

	/// <summary>
    /// Logs baker state (leaving)
    /// </summary>
    void BakerLeaves();

    /// <summary>
    /// Logs eater state (leaving)
    /// </summary>
	void EaterLeaves();

	/// <summary>
    /// Gets the time according to the 24hr system
    /// </summary>
    /// <returns>the current hour</returns>
	int GetHour();
}
