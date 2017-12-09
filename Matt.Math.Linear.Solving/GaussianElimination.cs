namespace Matt.Math.Linear.Solving
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Solves a system of equations
    /// </summary>
    public sealed class GaussianElimination
    {
        #region Private fields

        /// <summary>
        /// The list of coefficients
        /// </summary>
        private readonly IList<bool[]> _coefficients;

        /// <summary>
        /// The list of solutions
        /// </summary>
        private readonly IList<byte[]> _solutions;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="GaussianElimination"/> solver.
        /// Note the given parameters will be modified as <see cref="Solve"/> is invoked.
        /// </summary>
        public GaussianElimination(
            IList<bool[]> coefficients,
            IList<byte[]> solutions
            )
        {
            _coefficients = coefficients;
            _solutions = solutions;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to solve the system of equations given to the constructor.
        /// Returns null if no solution is available yet
        /// </summary>
        public IReadOnlyList<byte[]> Solve()
        {
            var numRows = _coefficients.Count;
            var numColumns = _coefficients[0].Length;
            var kMax = Math.Min(numRows, numColumns);

            // Put matrix into row echelon form
            for (var k = 0; k < kMax; k++) // O(n^2)
            {
                // Find the pivot point
                var iMax = 0;
                for (var i = k; i < numRows; i++) // O(n)
                {
                    if (!_coefficients[i][k])
                        continue;
                    iMax = i;
                    break;
                }
                if (!_coefficients[iMax][k])
                    return null;
                // Swap rows k and i_max
                if (iMax != k)
                {
                    SwapRows(iMax, k);
                }
                // XOR pivot with all rows below the pivot
                for (var i = k + 1; i < numRows; i++) // O(n)
                {
                    if (!_coefficients[i][k])
                        continue;
                    XorRows(k, i); // We can just XOR since we're dealing with Galois Fields
                }
            }

            // Put the matrix into reduced row echelon form using back substitution
            for (var k = kMax - 1; k > 0; k--) // O(n^2)
            {
                if (!_coefficients[k][k])
                    return null;
                // See which other rows need to be XOR'd with this one
                for (var i = k - 1; i >= 0; i--) // O(n)
                {
                    if (!_coefficients[i][k])
                        continue;
                    XorRows(k, i);
                }
            }

            // Make sure the top part of the coefficients matrix is the identity matrix and that the bottom part is only zeros
            for (var row = 0; row < numRows; row++) // O(n^2)
            {
                for (var column = 0; column < numColumns; column++) // O(n)
                {
                    if ((row == column) ^ _coefficients[row][column]) // There should be a coefficient and there's not, or there shouldn't be and there is
                        return null;
                }
            }

            // Make sure there are at least as many rows as there are columns
            if (numRows < numColumns)
                return null;
            
            // If we made it this far, then things have been solved
            var solution = new byte[numColumns][];
            for (var i = 0; i < numColumns; ++i)
            {
                solution[i] = (byte[]) _solutions[i].Clone();
            }
            return solution;
        }
        
        private static void Swap<T>(IList<T> list, int from, int to)
        {
            var temp = list[from];
            list[from] = list[to];
            list[to] = temp;
        }

        /// <summary>
        /// Swaps the coefficients and solutions between the two given rows
        /// </summary>
        private void SwapRows(int fromRow, int toRow)
        {
            if (fromRow == toRow)
                throw new Exception($"Are you sure you want to swap row {fromRow} with itself?");

            // Swap coefficients
            Swap(_coefficients, fromRow, toRow);

            // Swap solutions
            Swap(_solutions, fromRow, toRow);
        }
        
        /// <summary>
        /// Modifies this byte array by XOR'ing all the bytes with the given array
        /// </summary>
        private static void Xor(byte[] array, byte[] with)
        {
            if (array.Length != with.Length)
                throw new ArgumentException("The arrays must have the same length");
            for (var i = 0; i < array.Length; ++i)
                array[i] ^= with[i];
        }

        /// <summary>
        /// Modifies this boolean array by XOR'ing all the bits with the given array
        /// </summary>
        private static void Xor(bool[] array, bool[] with)
        {
            if (array.Length != with.Length)
                throw new ArgumentException("The arrays must have the same length");
            for (var i = 0; i < array.Length; ++i)
                array[i] ^= with[i];
        }

        /// <summary>
        /// XOR's the row at <paramref name="fromRow"/> into the row at <paramref name="toRow"/>, for both the coefficients and the solutions
        /// </summary>
        private void XorRows(int fromRow, int toRow)
        {
            if (fromRow == toRow)
                throw new Exception($"Are you sure you want to XOR row {fromRow} with itself?");

            // XOR coefficients
            Xor(_coefficients[toRow], _coefficients[fromRow]);

            // XOR solutions
            Xor(_solutions[toRow], _solutions[fromRow]);
        }

        #endregion
    }
}
