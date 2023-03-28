import { 
    ic,
    Opt,
    Principal,
    $init,
    $update,
    Record,
    int32,
    nat64,
    $query,
    int8
} from 'azle';

let owner: Opt<Principal> = null;

export type Position = Record<{
    x: int32;
    y: int32;
}>;

export type Player = Record<{
    owner: string;
    username: string;
    position: Position;
    lastTime: nat64;
}>;

export type Map = Record<{
    players: Player []
}>;

export type Error = Record<{
    msg: string
}>;

export type State = {
    players: {
        [key: string]: Player;
    };
};

// global state of players
let state : State = {
    players: {}
};

// init call
$init;
export function init() {
    ic.print ("init");    
    owner = ic.caller();
}

// login calls
export type Login = Record<{
   player: Player
   map: Map
}>;

type LoginResult = Record<{
   ok: Opt<Login>,
   error: Opt<Error>
}>;

$update;
export function login (): LoginResult 
{
    const caller = ic.caller().toString ();
    ic.print ("connection: " + caller);
    
    if (state.players[caller] === undefined) {
        state.players[caller] = {
            owner: caller,
            username: "",
            position: {
                x: 0,
                y: 0
            },
            lastTime: 0n
        };
    }

    var login : Login = {
        player: {
            owner: state.players[caller].owner,
            username: state.players[caller].username,
            position: state.players[caller].position,
            lastTime: state.players[caller].lastTime
        },
        map: {
            players: Object.entries (state.players).map ((player) => {
                return {
                        owner: player[1].owner,
                        username: player[1].username,
                        position: player[1].position,
                        lastTime: player[1].lastTime
                    }
                })
            }
        };

    return {
       ok: login,
       error: null
    }
}

// update the world map
export type UpdateMapResult = Record<{
    ok: Opt<Map>,
    error: Opt<Error>
}>

$query;
export function updateWorld(): UpdateMapResult 
{
    // send a map of all players
    // optimmize to send players in region
    var map : Map = {
        players: Object.entries (state.players).map ((player) => {
            return {
                    owner: player[1].owner,
                    username: player[1].username,
                    position: player[1].position,
                    lastTime: player[1].lastTime
                }
            })
        };

    return {
        ok: map,
        error: null
    }
}

// Move message for player
export type moveMsg = Record<{
    dir: int32;
}>;

export type MovePlayer = Record<{
    player: Player,
    map: Map,
    time: nat64
}>;

export type MovePlayerResult = Record<{
    ok: Opt<MovePlayer>,
    error: Opt<Error>
}>

$update;
export function movePlayer (moveMsg : moveMsg): MovePlayerResult
{
    const caller = ic.caller().toString ();

    // create player if he is new
    if (state.players[caller] === undefined) {
        ic.print ("No Player");
        return {
            ok: null,
            error: { msg: "No such player"}
        };
    }

    let player = state.players[caller];

    let time : nat64 = ic.time (); // nano seconds
    time = time / 1000000n;

    // check the players move time
    if (player.lastTime + 5000n > time)
    {
        ic.print ("Can't Move yet");

        return {
            ok: null,
            error: {msg: "Player unable to move"}
        };
    }

    // move player
    if (moveMsg.dir == 0 && player.position.x < 10)
       player.position.x++;
    else if (moveMsg.dir == 1 && player.position.x > -10)
       player.position.x--;
    else if (moveMsg.dir == 2 && player.position.y < 10)
       player.position.y++;
    else if (moveMsg.dir == 3 && player.position.y > -10)
       player.position.y--;

    // force player to wait 10s
    player.lastTime = time + 10000n;

    var moveResult : MovePlayer = {
        time: time,
        player: {
            owner: player.owner,
            username: player.username,
            position: player.position,
            lastTime: player.lastTime
        },
        map: {
            players: Object.entries (state.players).map ((player) => {
                return {
                        owner: player[1].owner,
                        username: player[1].username,
                        position: player[1].position,
                        lastTime: player[1].lastTime
                    }
                })
            }
        };

    return {
        ok: moveResult,
        error: null
    }
 }
 