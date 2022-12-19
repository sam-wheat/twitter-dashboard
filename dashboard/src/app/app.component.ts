import { Component } from '@angular/core';
import { FeedStats } from './model/feedstats';
import { DashboardService } from './services/dashboard.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Twitter dashboard';
  FeedStats! : FeedStats | null;
  IsFeedStarted! : boolean | null; 

  constructor(private dashboardService: DashboardService)
  {
    
  }

  async ngOnInit() 
  {
    await this.dashboardService.StartHubListener();
    
    this.dashboardService.FeedStats.subscribe(x => 
      {
              this.FeedStats = x;
      });

    this.dashboardService.GetFeedStatus().subscribe(x => {
      this.IsFeedStarted = x; 
    });
  }

  StartFeed_Click() 
  {
    this.IsFeedStarted = null;
    this.dashboardService.StartFeed().subscribe(x => {
      this.IsFeedStarted = x; 
      this.RefreshStats_Click(); 
    });
  }

  StopFeed_Click() 
  {
    this.IsFeedStarted = null;
    this.dashboardService.StopFeed().subscribe(x => {
      this.IsFeedStarted = x;
       
      if(this.IsFeedStarted == false)
        this.FeedStats = null;
    });
  }

  RefreshStats_Click() 
  {
    this.dashboardService.RefreshStats().subscribe(x => {
      // this call does not return anything - it tells the server to broadcast now over signal r.
    });
  }
}
