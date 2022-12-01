import { Component } from '@angular/core';
import RemoteFileSystemProvider from 'devextreme/file_management/remote_provider';
import { environment } from 'src/environments/environment';

@Component({
  templateUrl: 'home.component.html',
  styleUrls: [ './home.component.scss' ]
})

export class HomeComponent {
  fileProvider: any | null;
  
  constructor() {
    this.fileProvider = new RemoteFileSystemProvider({
      endpointUrl: environment.baseUrl + '/api/files/manage'
    });
  }
}
