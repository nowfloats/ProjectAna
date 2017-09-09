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
    MdSnackBarModule,
    MdCardModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/nodeeditor/nodeeditor.component';

import { ChatFlowService } from './services/chatflow.service';
import { GlobalsService } from './services/globals.service';
import { SettingsService } from './services/settings.service';
import { ChatServerManagerComponent } from './components/chat-server-manager/chat-server-manager.component';
import { ProjectsManagerComponent } from './components/projects-manager/projects-manager.component';
import { EllipsisPipe } from './pipes/ellipsis.pipe';

@NgModule({
    declarations: [
        AppComponent,
        ChatFlowComponent,
        NodeEditorComponent,
        ChatServerManagerComponent,
        ProjectsManagerComponent,
        EllipsisPipe
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
        MdCardModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'connections', pathMatch: 'full' },
            { path: 'designer/:id', component: ChatFlowComponent },
            { path: 'projects', component: ProjectsManagerComponent },
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
