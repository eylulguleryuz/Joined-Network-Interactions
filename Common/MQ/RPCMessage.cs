namespace Commons.MQs;


/// <summary>
/// Wrapper for RPC calls and responses.
/// </summary>
public class RPCMessage
{
	/// <summary>
	/// Action type.
	/// </summary>
	public String Action { get; set; }

	/// <summary>
	/// Action data.
	/// </summary>
	public String Data { get; set; }
}