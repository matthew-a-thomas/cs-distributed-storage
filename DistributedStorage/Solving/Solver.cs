namespace DistributedStorage.Solving
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Encoding;

    /// <summary>
    /// Solves for data
    /// </summary>
    internal class Solver : ISolver
    {
        /// <summary>
        /// Our working copy of coefficients
        /// </summary>
        private readonly IList<bool[]> _coefficients = new List<bool[]>();

        /// <summary>
        /// Our working copy of solutions which have been received so far
        /// </summary>
        private readonly IList<byte[]> _solutions = new List<byte[]>();

        /// <summary>
        /// Our gaussian solver
        /// </summary>
        private readonly GaussianElimination _gaussian;

        /// <summary>
        /// The manifest which should have produced the data we're solving for
        /// </summary>
        private readonly Manifest _solveFor;

        /// <summary>
        /// Something which is able to produce new manifests
        /// </summary>
        private readonly IManifestFactory _manifestFactory;

        /// <summary>
        /// Creates a new <see cref="Solver"/> which will solve for the data which produced this <see cref="Manifest"/>
        /// </summary>
        public Solver(Manifest solveFor, IManifestFactory manifestFactory)
        {
            _solveFor = solveFor;
            _manifestFactory = manifestFactory;
            _gaussian = new GaussianElimination(_coefficients, _solutions);
        }

        /// <summary>
        /// Tries to recover the original data for the <see cref="Manifest"/> for the data which produced this <paramref name="slice"/> and all past given slices.
        /// If successful, true is returned and <paramref name="solution"/> will contain the original data.
        /// If the internal solver was able to reconstruct the data but the <see cref="Manifest"/> doesn't match, then false is returned and <paramref name="solution"/> will contain that data.
        /// Otherwise, false is returned and <paramref name="solution"/> is set to null
        /// </summary>
        public bool TrySolve(Slice slice, out byte[] solution)
        {
            // Incorporate the given slice as new information for the solver
            _coefficients.Add((bool[])slice.Coefficients.Clone());
            _solutions.Add((byte[])slice.EncodingSymbol.Clone());

            // Wait until we have enough solutions to potentially solve
            if (_solutions.Count < _solveFor.SliceHashes.Length)
            {
                solution = null;
                return false;
            }

            // Ask the solver to try and solve it
            var attempt = _gaussian.Solve();
            if (attempt == null)
            {
                solution = null;
                return false;
            }

            // Put everything together into one block of bytes
            {
                var totalNumBytes = attempt.Select(x => x.Length).Sum();
                if (totalNumBytes < _solveFor.Length) // Ensure we solved the same number of bytes as the manifest said there would be
                {
                    solution = null;
                    return false;
                }

                solution = new byte[_solveFor.Length];
                var i = 0;
                foreach (var piece in attempt)
                {
                    Array.Copy(piece, 0, solution, i, Math.Min(piece.Length, _solveFor.Length - i));
                    i += piece.Length;
                }
            }

            // Now verify that the solution's manifest is correct
            var newManifest = _manifestFactory.CreateManifestFrom(solution, _solveFor.SliceHashes.Length);
            var idsMatch = newManifest.Id.HashCode.SequenceEqual(_solveFor.Id.HashCode);
            var sliceHashMatches = newManifest.SliceHashes.Select((x, i) => x.HashCode.SequenceEqual(_solveFor.SliceHashes[i].HashCode)).ToList();

            // Return true only if the ID matches and all the slice hashes match
            return idsMatch && sliceHashMatches.All(x => x);
        }
    }
}
