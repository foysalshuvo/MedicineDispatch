using System;

public class Drone
{
	public int Id { get; set; }
	public string SerialNumber { get;set }
	public string Model { get; set; }
	public double Weight { get; set; }
	public double Battery {get; set; }
	public string State { get; set; }
	public string CreatedBy {get; set; }
	public DateTime CreatedDate { get; set; }
	public string UpdateBy {get; set; }
	public DateTime UpdateDate { get; set; }
	public bool IsActive { get; set; }
}
