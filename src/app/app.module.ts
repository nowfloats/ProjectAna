import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
    MdButtonModule,
    MdMenuModule,
    MdSidenavModule,
    MdInputModule,
    MdDialogModule,
    MdSelectModule,
    MdCheckboxModule,
    MdTabsModule,
    MdIconModule,
    MdTooltipModule,
    MdExpansionModule,
    MdProgressSpinnerModule,
    MdSnackBarModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/nodeeditor/nodeeditor.component';

import { ChatFlowService } from './services/chatflow.service';
import { GlobalsService } from './services/globals.service';
import { SettingsService } from './services/settings.service';
import { ChatServerManagerComponent } from './components/chat-server-manager/chat-server-manager.component';

@NgModule({
    declarations: [
        AppComponent,
        ChatFlowComponent,
        NodeEditorComponent,
        ChatServerManagerComponent
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
        MdIconModule,
        MdTooltipModule,
        MdExpansionModule,
        MdProgressSpinnerModule,
        MdSnackBarModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'connections', pathMatch: 'full' },
            { path: 'designer', component: ChatFlowComponent },
            { path: 'connections', component: ChatServerManagerComponent },
            { path: '**', redirectTo: 'connections' }
        ])
    ],
    providers: [
        ChatFlowService,
        GlobalsService,
        SettingsService
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
