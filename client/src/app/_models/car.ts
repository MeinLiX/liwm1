import { RacingTransmissionRange } from "./racingTransmissionRange";

export class Car {
    id: number;
    racerName: string;
    isReady: boolean;
    isFinished: boolean;
    x: number;
    y: number;
    dx: number;
    dy: number;
    lap: number;
    transmission: number;   
    boostMode: RacingTransmissionRange;

    constructor(x: number, y: number, id: number, racerName: string) {
        this.x = x;
        this.y = y;
        this.id = id;
        this.racerName = racerName;
        this.dx = 0;
        this.dy = 0;
        this.lap = 1;
        this.isFinished = false;
        this.isReady = false;
        this.transmission = 0;
        this.boostMode = RacingTransmissionRange.Bad;
    }
}