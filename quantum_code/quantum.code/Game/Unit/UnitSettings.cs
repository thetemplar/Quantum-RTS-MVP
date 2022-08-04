namespace Quantum
{
	using Photon.Deterministic;
	using Quantum.Inspector;
	using Quantum.Prototypes;

	[System.Serializable]
	public class UnitSettings
	{
		[Header("Unit")]
		public int                      UnitCount = 1;

		[Space]
		[Header("Health")]
		public FP                       BaseHealth;

		[Header("Movement")]
		public FP                       MovementSpeed;
	}
}
