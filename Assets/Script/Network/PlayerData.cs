using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int characterId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public TeamID teamId;


    public bool Equals(PlayerData other)
    {
        return
            clientId == other.clientId &&
            characterId == other.characterId &&
            playerName == other.playerName &&
            playerId == other.playerId &&
            teamId == other.teamId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref teamId);
    }

}

[Serializable]
public enum TeamID
{
    TeamA, TeamB,
}