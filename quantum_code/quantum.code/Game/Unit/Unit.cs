namespace Quantum
{
	using System;
	using Photon.Deterministic;

	public unsafe partial struct Unit
	{
		public void Initialize(Frame frame, PlayerRef owner, EntityRef entity)
		{
			Owner = owner;

			if (frame.Unsafe.TryGetPointer<Movement>(entity, out var targetMovement) == true)
			{
				targetMovement->MovePosition = new FPVector2(1,1);
			}
		}


		public void Update(Frame frame, EntityRef entity)
		{
		}
	}
}
