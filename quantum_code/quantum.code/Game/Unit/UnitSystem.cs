namespace Quantum
{
	unsafe class UnitSystem : SystemMainThread
	{
		public override void Update(Frame frame)
		{
			foreach (var pair in frame.Unsafe.GetComponentBlockIterator<Unit>())
			{
				pair.Component->Update(frame, pair.Entity);
			}
		}
	}
}
