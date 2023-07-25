using System;
using System.Linq;
using UnityEngine;
public class Motion : MonoBehaviour
{
	public double speedPerSec = 0.5;
	public int fps = 10;
	public double[] points = { 3.5 };
	public double output;
	private double distLeft, distTravelled;
	private double updateEvery;
	private double fpsCap;
	public void Awake()
	{
		distLeft = 0.0;
		distTravelled = 0.0;
		
		for(int a = 1, A = points.Length; a < A; ++a)
			distLeft += Math.Abs(points[a] - points[a - 1]);
		
		updateEvery = 1.0 / fps;
		fpsCap = updateEvery;
		output = points[0];
	}
	private static double map(double value, double minValue, double maxValue, double minOut, double maxOut)
	{
		return minOut + (value - minValue) / (maxValue - minValue) * (maxOut - minOut);
	}
	public void Update()
	{
		if(distLeft <= 0.0)
			return;
		
		double dt = Time.deltaTime;
		fpsCap += dt;
		if(fpsCap < updateEvery)
			return;
		fpsCap -= updateEvery;
		
		// Frame update
		double lenOfThisFrame = speedPerSec * updateEvery;
		distTravelled += lenOfThisFrame;
		double dist;
		while(points.Length > 1 && distTravelled >= (dist = Math.Abs(points[1] - points[0]))) // We have reached a new point
		{
			points = points.Skip(1).ToArray();
			distTravelled -= dist;
			distLeft -= dist;
		}
		if(points.Length == 1)
		{
			output = points[0];
			return;
		}
		double percentTravelled = distTravelled / dist;
		output = map(percentTravelled, 0.0, 1.0, points[0], points[1]);
	}
}