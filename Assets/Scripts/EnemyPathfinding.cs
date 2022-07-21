using UnityEngine;
using System.Collections;

namespace Pathfinding
{
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class EnemyPathfinding : VersionedMonoBehaviour
	{
		/// <summary>The object that the AI should move to</summary>
		public Transform target;
		private Transform player;
		[SerializeField]
		private float fleeDistance = 8;
		[SerializeField]
		private float chargeDistance = 9;
		IAstarAI ai;

		void OnEnable()
		{
			ai = GetComponent<IAstarAI>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;

			player = GameObject.FindWithTag("Player").transform;

			if (target == null)
			{
				target = GameObject.FindWithTag("Target").transform;
			}
		}

		void OnDisable()
		{
			if (ai != null) ai.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update()
		{
			if (player != null)
			{
				if (Vector2.Distance(transform.position, target.position) < chargeDistance)
				{
					// enemy is close to the target
					if (target != null && ai != null) ai.destination = target.position;
				}
				else if (Vector2.Distance(transform.position, player.position) < fleeDistance)
				{
					// enemy is too close to player
					if (target != null && ai != null)
					{
						Vector3 fleeDirection = (player.position - transform.position) * -1;
						fleeDirection = fleeDirection.normalized + target.position.normalized;
						Vector3 newTarget = transform.position + (fleeDirection.normalized * 20);

						ai.destination = newTarget;
					}
				}
				else
				{
					if (target != null && ai != null) ai.destination = target.position;
				}
			}
            else
            {
				if (ai != null) ai.destination = GameObject.FindWithTag("Target").transform.position;
			}
		}
	}
}
