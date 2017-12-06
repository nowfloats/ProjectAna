const shell = require('shelljs');

shell.rm('-R', 'src/node_modules'); //This will create problems with typescript compilation of angular 4 app

shell.echo('Building studio project...');
shell.exec('ng build --prod --aot=false');

shell.echo('Changing to simulator project...');
if (shell.cd('../ana-web-chat/').code == 0) {

	shell.echo('Building simulator project...');
	shell.exec('ng build --prod');

	shell.echo('Changing back to studio project...');
	shell.cd('../ana-studio-web/');

	shell.echo('Copying simulator to studio dist..');
	shell.cp('-R', '../ana-web-chat/dist', 'dist/simulator');

	shell.echo('Updating simulator index.html base tag...');
	shell.sed('-i', '<base href="./">', '<base href="./simulator">', 'dist/simulator/index.html');

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