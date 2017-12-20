import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatFlowComponent } from './chatflow/chatflow.component';
import { NodeEditorComponent } from './nodeeditor/nodeeditor.component';
import { SimulatorFrameComponent } from './simulator-frame/simulator-frame.component';
import { LandingComponent } from './landing/landing.component';

import { SharedModule } from '../shared/shared.module';

export const STUDIO_ROUTES: Routes = [
	{
		path: "",
		component: LandingComponent
	},
	{
		path: "designer",
		component: ChatFlowComponent
	}
];

@NgModule({
	declarations: [
		ChatFlowComponent,
		NodeEditorComponent,
		SimulatorFrameComponent,
		LandingComponent
	],
	entryComponents: [
		NodeEditorComponent
	],
	imports: [
		SharedModule
	]
})
export class StudioModule { }

