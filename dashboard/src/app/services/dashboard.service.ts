import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { domainConstants } from '../domain/domain-constants';
import { environment } from '../environments/environment.dev';
import { FeedStats } from '../model/feedstats';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  
  private hub : HubConnection;
  FeedStats : Subject<FeedStats> = new Subject();

 
  constructor(private http: HttpClient) 
  {
    this.hub = new HubConnectionBuilder().withUrl(environment.APIServer_hub_URL).build();
  }

  async StartHubListener() 
  {
    await this.hub.start()
    .then(() => console.log("connection to SignalR server established."))
    .catch((err : string) => console.error(err));
    
    // Wire up the listener
    
    await this.hub.on(domainConstants.HubChannelName, (username: string, message: FeedStats) => {
      this.FeedStats.next(message);
    });
  }

  StartFeed() : Observable<boolean>
  {
    var url = environment.APIServer_dashboard_URL + 'StartFeed';
    return this.http.get<boolean>(url);
  }

  StopFeed() : Observable<boolean>
  {
    var url = environment.APIServer_dashboard_URL + 'StopFeed';
    return this.http.get<boolean>(url);
  }

  GetFeedStatus() : Observable<boolean>
  {
    var url = environment.APIServer_dashboard_URL + 'FeedStatus';
    return this.http.get<boolean>(url);
  }

  RefreshStats() : Observable<any>
  {
    var url = environment.APIServer_dashboard_URL + 'BroadcastStats';
    return this.http.get(url);

  }


  // Send a message to the server
  Send(msg:string)
  {
    this.hub.send("newMessage", 456, msg)
      .catch((err: string) => { 
        console.error(err);
      });
  }
}
