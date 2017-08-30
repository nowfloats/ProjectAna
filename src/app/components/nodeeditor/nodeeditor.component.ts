import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import 'rxjs/add/operator/filter';

@Component({
  selector: 'app-nodeeditor',
  templateUrl: './nodeeditor.component.html',
  styleUrls: ['./nodeeditor.component.css']
})
export class NodeEditorComponent {

  nodeId: string;
  constructor(public router: ActivatedRoute) {
    this.router.queryParams
      .filter(params => params.order)
      .subscribe(params => {
        console.log(params);
      });
  }
}


