import { Component } from '@angular/core';
import { GlobalsService } from './services/globals.service';
@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(public global: GlobalsService) { }

    loading() {
        return this.global.loading;
    }
    hideLoading() {
        this.global.loading = false;
    }
    pageName() {
        return this.global.currentPageName;
    }
}
