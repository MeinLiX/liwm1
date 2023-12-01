import { RacingTransmissionRange } from "./racingTransmissionRange";

export class Car {
    id: number;
    type: CarType;
    image?: HTMLImageElement;
    racerName: string;
    isReady: boolean;
    isFinished: boolean;
    x: number;
    y: number;
    dx: number;
    dy: number;
    width: number;
    height: number;
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
        this.width = 0;
        this.height = 0;
        this.type = CarType.Red;
    }
}

export interface BackendCar {
    id: number;
    racerName: string;
    racingCarBoostMode: RacingTransmissionRange;
    isReady: boolean;
    isFinished: boolean;
}

export enum CarType {
    Red,
    Black,
    Blue,
    Cyan,
    Gray,
    Green,
    Orange,
    Yellow
}