import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
    MdButtonModule,
    MdMenuModule,
    MdSidenavModule,
    MdInputModule,
    MdDialogModule,
    MdSelectModule,
    MdCheckboxModule,
    MdTabsModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/nodeeditor/nodeeditor.component';
import { HttpClientModule } from '@angular/common/http';

import { ChatFlowService } from './services/chatflow.service';
import { GlobalsService } from './services/globals.service';

@NgModule({
    declarations: [
        AppComponent,
        ChatFlowComponent,
        NodeEditorComponent
    ],
    imports: [
        BrowserModule,
        CommonModule,
        HttpModule,
        FormsModule,
        MdButtonModule,
        MdMenuModule,
        MdSidenavModule,
        MdInputModule,
        MdDialogModule,
        HttpClientModule,
        BrowserAnimationsModule,
        MdSelectModule,
        MdCheckboxModule,
        MdTabsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'designer', pathMatch: 'full' },
            { path: 'designer', component: ChatFlowComponent },
            { path: '**', redirectTo: 'designer' }
        ])
    ],
    providers: [
        ChatFlowService,
        GlobalsService
    ],
    bootstrap: [AppComponent],
    schemas: [
        NO_ERRORS_SCHEMA
    ],
    entryComponents: [
        NodeEditorComponent
    ]
})
export class AppModule { }
