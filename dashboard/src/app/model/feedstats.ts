export class FeedStats 
{
    constructor() { }

    SnapshotTime : Date = new Date();
    ServerStartTime : Date = new Date();
    TweetCount : number = 0;
    TopHashtags : Array<string> = new Array<string>();
    UpTime : number = 0;
    AvgTweetsPerMin : number = 0;
}