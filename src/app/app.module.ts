import { BrowserModule, Title } from '@angular/platform-browser';
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
	MdCardModule,
	MdListModule,
	MdGridListModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/nodeeditor/nodeeditor.component';

import { ChatFlowService } from './services/chatflow.service';
import { GlobalsService } from './services/globals.service';
import { SettingsService } from './services/settings.service';
import { InfoDialogService } from './services/info-dialog.service';
import { SimulatorService } from './services/simulator.service';

import { ChatServerManagerComponent } from './components/chat-server-manager/chat-server-manager.component';
import { EllipsisPipe } from './pipes/ellipsis.pipe';
import { StartupComponent } from './components/startup/startup.component';
import { PublishDialogComponent } from './components/publish-dialog/publish-dialog.component';
import { InfoDialogComponent } from './components/info-dialog/info-dialog.component';
import { SimulatorFrameComponent } from './components/simulator-frame/simulator-frame.component';

@NgModule({
	declarations: [
		AppComponent,
		ChatFlowComponent,
		NodeEditorComponent,
		ChatServerManagerComponent,
		EllipsisPipe,
		StartupComponent,
		PublishDialogComponent,
		InfoDialogComponent,
		SimulatorFrameComponent
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
		MdListModule,
		MdGridListModule,
		RouterModule.forRoot([
			{ path: '', redirectTo: 'startup', pathMatch: 'full' },
			{ path: 'designer', component: ChatFlowComponent },
			{ path: 'startup', component: StartupComponent },
			{ path: '**', redirectTo: 'startup' }
		])
	],
	providers: [
		ChatFlowService,
		GlobalsService,
		SettingsService,
		InfoDialogService,
		Title,
		SimulatorService
	],
	bootstrap: [AppComponent],
	schemas: [
		NO_ERRORS_SCHEMA
	],
	entryComponents: [
		NodeEditorComponent,
		ChatServerManagerComponent,
		PublishDialogComponent,
		InfoDialogComponent
	]
})
export class AppModule { }
