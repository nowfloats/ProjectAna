const shell = require('shelljs');
let skipSimulator = (process.argv.indexOf('--skip-sim') != -1);
let devBuild = (process.argv.indexOf('--dev') != -1);

if (devBuild) {
	shell.echo('Building in dev mode...');
}

shell.rm('-R', 'src/node_modules'); //This will create problems with typescript compilation of angular 4 app

shell.echo('Building studio project...');
shell.exec(`ng build ${devBuild ? '' : '--prod'} --aot=false`);

shell.echo('Changing to simulator project...');
if (shell.cd('../ana-web-chat/').code == 0) {

	if (!skipSimulator) {
		shell.echo('Building simulator project...');
		shell.exec(`ng build ${devBuild ? '' : '--prod'}`);
	}

	shell.echo('Changing back to studio project...');
	shell.cd('../ana-studio-web/');

	if (!skipSimulator) {
		shell.echo('Copying simulator to studio dist..');
		shell.cp('-R', '../ana-web-chat/dist', 'dist/simulator');

		shell.echo('Updating simulator index.html base tag...');
		shell.sed('-i', '<base href="./">', '<base href="./simulator">', 'dist/simulator/index.html');
	}

	if (shell.cd('../ana-analytics-dashboard/').code == 0) {
		shell.echo('Building analytics dashboard project...');
		shell.exec(`ng build ${devBuild ? '' : '--prod'} --aot=false`);

		shell.echo('Changing back to studio project...');
		shell.cd('../ana-studio-web/');

		shell.echo('Copying analytics dashboard to studio dist..');
		shell.cp('-R', '../ana-analytics-dashboard/dist', 'dist/analytics-dashboard');
	} else {
		shell.echo('Project `ana-analytics-dashboard` not found! Make sure it is present adjacent to ana-studio-web. Skipping analytics dashboard!');
	}

	shell.echo('Packing Ana Studio into an electron app...');
	shell.rm('-R', 'release');
	shell.cd('dist');
	shell.exec('electron-builder');
	shell.cd('../dist');
	shell.echo('Done');
} else {
	shell.echo('Project `ana-web-chat` not found! Make sure it is present adjacent to ana-studio-web. ');
	shell.echo('Aborted');
}