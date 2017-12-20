import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
	MatButtonModule,
	MatMenuModule,
	MatSidenavModule,
	MatInputModule,
	MatDialogModule,
	MatSelectModule,
	MatCheckboxModule,
	MatTabsModule,
	MatIconModule,
	MatTooltipModule,
	MatExpansionModule,
	MatProgressSpinnerModule,
	MatSnackBarModule,
	MatCardModule,
	MatListModule,
	MatGridListModule,
	MatProgressBarModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/studio/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/studio/nodeeditor/nodeeditor.component';

import { ChatFlowService } from './services/chatflow.service';
import { GlobalsService } from './services/globals.service';
import { SettingsService } from './services/settings.service';
import { InfoDialogService } from './services/info-dialog.service';
import { SimulatorService } from './services/simulator.service';

import { ChatServerManagerComponent } from './components/common/chat-server-manager/chat-server-manager.component';
import { EllipsisPipe } from './pipes/ellipsis.pipe';
import { StudioLandingComponent } from './components/studio/studio-landing/studio-landing.component';
import { PublishDialogComponent } from './components/common/publish-dialog/publish-dialog.component';
import { InfoDialogComponent } from './components/common/info-dialog/info-dialog.component';
import { SimulatorFrameComponent } from './components/studio/simulator-frame/simulator-frame.component';
import { LoadingIndicatorComponent } from './components/common/loading-indicator/loading-indicator.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/common/login/login.component';
//import { ManageUsersComponent } from './components/manage-users/manage-users.component';

@NgModule({
	declarations: [
		AppComponent,
		ChatFlowComponent,
		NodeEditorComponent,
		ChatServerManagerComponent,
		EllipsisPipe,
		StudioLandingComponent,
		PublishDialogComponent,
		InfoDialogComponent,
		SimulatorFrameComponent,
		LoadingIndicatorComponent,
		HomeComponent,
		LoginComponent
	],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		CommonModule,
		HttpModule,
		FormsModule,
		MatButtonModule,
		MatMenuModule,
		MatSidenavModule,
		MatInputModule,
		MatDialogModule,
		HttpClientModule,
		MatSelectModule,
		MatCheckboxModule,
		MatTabsModule,
		MatIconModule,
		MatTooltipModule,
		MatExpansionModule,
		MatProgressSpinnerModule,
		MatProgressBarModule,
		MatSnackBarModule,
		MatCardModule,
		MatListModule,
		MatGridListModule,
		RouterModule.forRoot([
			{ path: '', redirectTo: 'home', pathMatch: 'full' },
			{ path: 'designer', component: ChatFlowComponent },
			{ path: 'studio-landing', component: StudioLandingComponent },
			//{ path: 'manage-users', component: ManageUsersComponent },
			{ path: 'home', component: HomeComponent },
			{ path: '**', redirectTo: 'home' }
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
