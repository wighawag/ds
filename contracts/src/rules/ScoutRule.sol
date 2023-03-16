// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import {State} from "cog/State.sol";
import {Context, Rule} from "cog/Dispatcher.sol";
import {Context, Rule} from "cog/Dispatcher.sol";

import {
    Schema,
    Node,
    BiomeKind,
    ResourceKind,
    AtomKind,
    TileUtils,
    TRAVEL_SPEED,
    DEFAULT_ZONE
} from "@ds/schema/Schema.sol";
import {Actions} from "@ds/actions/Actions.sol";

using Schema for State;

error NoScoutNotOwner();
error NoScoutAlreadyDiscovered();
error NoScoutUnadjacent();

contract ScoutRule is Rule {
    function reduce(State state, bytes calldata action, Context calldata ctx) public returns (State) {
        if (bytes4(action) == Actions.SCOUT_SEEKER.selector) {
            // decode the action
            (uint32 sid, int16[3] memory coords) = abi.decode(action[4:], (uint32, int16[3]));

            // encode the full seeker node id
            bytes24 seeker = Node.Seeker(sid);

            // check that sender owns seeker
            if (state.getOwner(seeker) != Node.Player(ctx.sender)) {
                revert NoScoutNotOwner();
            }

            // encode destination tile
            bytes24 targetTile = Node.Tile(DEFAULT_ZONE, coords[0], coords[1], coords[2]);

            // fail if already discovered
            if (state.getBiome(targetTile) == BiomeKind.DISCOVERED) {
                revert NoScoutAlreadyDiscovered();
            }

            // fetch the seeker's current location
            (bytes24 seekerTile) = state.getCurrentLocation(seeker, ctx.clock);

            // check that target is adjacent to seeker
            if (TileUtils.distance(seekerTile, targetTile) != 1) {
                revert NoScoutUnadjacent();
            }

            // do the reveal
            state.setBiome(targetTile, BiomeKind.DISCOVERED);

            // [temp] randomly spawn a bag with some wood in it
            // on the tile as a crappy temp resource faucet
            _tempSpawnResourceBag(state, targetTile, coords);
        }

        return state;
    }

    uint8 private _resourceSpawnCount = 0; // Used with modulo for round robin spawning of resouces
    uint8 private constant NUM_RESOURCE_KINDS = 3;

    function _tempSpawnResourceBag(State state, bytes24 targetTile, int16[3] memory coords) private {
        uint64 bagID = uint64(uint256(keccak256(abi.encode(coords))));
        if (uint8(bagID) < 50) {
            bytes24 bag = Node.Bag(bagID);

            ResourceKind resourceKind = ResourceKind((_resourceSpawnCount % NUM_RESOURCE_KINDS) + 1);

            state.setItemSlot(bag, 0, Node.Resource(resourceKind), 100);
            state.setEquipSlot(targetTile, 0, bag);

            _resourceSpawnCount++;
        }
    }
}