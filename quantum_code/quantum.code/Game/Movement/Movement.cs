﻿namespace Quantum
{
	using Photon.Deterministic;

	public unsafe partial struct Movement
	{
		// PUBLIC METHODS

		public void Initialize(Frame frame, EntityRef entity, UnitSettings settings)
		{
			MovementSpeed = settings.MovementSpeed;
		}

		public void Update(Frame frame, EntityRef entity)
		{
			UpdateNavMeshMove(frame, entity);

			BusyTime -= frame.DeltaTime;
		}

		public void OnNavMeshMoveAgent(Frame frame, EntityRef entity, FPVector2 desiredDirection)
		{
			var steeringAgent = frame.Unsafe.GetPointer<NavMeshSteeringAgent>(entity);
			var transform     = frame.Unsafe.GetPointer<Transform2D>(entity);
			var velocity      = (BusyTime <= FP._0) ? desiredDirection * frame.DeltaTime * steeringAgent->MaxSpeed : FPVector2.Zero;

			transform->Position     += velocity;
			steeringAgent->Velocity  = velocity;
		}

		public void Stop(Frame frame, EntityRef entity)
		{
			var pathfinder = frame.Unsafe.GetPointer<NavMeshPathfinder>(entity);
			pathfinder->Stop(frame, entity, true);

			var transform = frame.Unsafe.GetPointer<Transform2D>(entity);
			MovePosition  = transform->Position;
		}

		// PRIVATE METHODS

		private void UpdateNavMeshMove(Frame frame, EntityRef entity)
		{
			var pathfinder = frame.Unsafe.GetPointer<NavMeshPathfinder>(entity);

			if ((pathfinder->Target.XZ - MovePosition).SqrMagnitude > FP._0_25)
			{
				Log.Debug("frame" + frame.DumpFrame());
				Log.Debug("MovePosition" + MovePosition.ToString());
				Log.Debug("frame.Context.CurrentNavMesh" + frame.Context.CurrentNavMesh.Path + " " + frame.Context.CurrentNavMesh.Vertices);
				pathfinder->SetTarget(frame, MovePosition.XOY, frame.Context.CurrentNavMesh);
			}

			var currentWaypoint  = LookPosition;

			if (pathfinder->WaypointCount > 0 && pathfinder->CurrentWaypointHasFlag(frame, Navigation.WaypointFlag.Target) == false)
			{
				currentWaypoint = pathfinder->GetWaypoint(frame, pathfinder->WaypointIndex).XZ;
			}

			var steeringAgent = frame.Unsafe.GetPointer<NavMeshSteeringAgent>(entity);
			var speed         = FP._0;

			if (BusyTime <= FP._0)
			{
				speed = MovementSpeed;
			}

			steeringAgent->MaxSpeed = speed;

		//	UpdateRotation(frame, transform, currentWaypoint);
		}

		private void UpdateRotation(Frame frame, Transform2D* transform, FPVector2 lookPosition)
		{
			var desiredDirection = lookPosition - transform->Position;

			var velocitySqrMagnitude = desiredDirection.SqrMagnitude;
			if (velocitySqrMagnitude > FP._0)
			{
				var desiredRotation = FPVector2.Angle(FPVector2.Up, desiredDirection) * FP.Deg2Rad * (FPVector2.Cross(FPVector2.Up, desiredDirection) > 0 ? 1 : -1);
				transform->Rotation = FPMath.LerpRadians(transform->Rotation, desiredRotation, frame.DeltaTime * FP._10);
			}
		}
	}
}
