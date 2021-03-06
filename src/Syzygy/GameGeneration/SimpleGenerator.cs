using System.Collections.Generic;
using Bearded.Utilities.Math;
using Bearded.Utilities.SpaceTime;
using Syzygy.Game;
using Syzygy.Game.Astronomy;

namespace Syzygy.GameGeneration
{
    sealed class SimpleGenerator : IGenerator
    {
        public IEnumerable<IGenerationInstruction> Generate(IList<Id<Player>> playerIds)
        {
            var idMan = new IdManager();

            var sun = idMan.GetNext<IBody>();

            yield return new NewFixedBodyInstruction(sun, new Position2(), Radius.FromValue(1), 1);

            var orbitRadius = 7.U();
            var orbitStep = 4.U();

            foreach (var pId in playerIds)
            {
                var planet = idMan.GetNext<IBody>();

                yield return new NewOrbitingBodyInstruction(planet, sun,
                    Radius.FromValue(orbitRadius), Direction2.Zero, Radius.FromValue(0.5f), 0.25f, 100);

                yield return new AssignPlayerToBodyInstruction(pId, planet);

                orbitRadius += orbitStep;
            }


        }
    }
}
