namespace TicTacToe.Core.Models;

/// <summary>
/// Represents a single position on the board.
/// </summary>
/// <param name="Index">The zero-based board index.</param>
/// <param name="Mark">The mark currently assigned to the space.</param>
public sealed record BoardSpace(int Index, PlayerMark? Mark);