using TicTacToe.Core.Models;

namespace TicTacToe.Core.Contracts;

/// <summary>
/// Represents a client request to apply one authoritative move.
/// </summary>
/// <param name="SpaceIndex">The zero-based board index.</param>
/// <param name="ActingPlayer">The player believed to be acting.</param>
/// <param name="ExpectedRevision">The last revision observed by the client.</param>
public sealed record MoveRequestDto(int SpaceIndex, PlayerMark ActingPlayer, int ExpectedRevision);