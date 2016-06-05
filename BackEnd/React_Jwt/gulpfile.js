var gulp = require("gulp");
var runner = require("gulp-run");
var utility = require('gulp-util');
var webpack = require('webpack');
var webpackConfig = require('../../FrontEnd/webpack.config.js');

gulp.task("compile:js", function (callback) {
    var myConfig = Object.create(webpackConfig);
    runner('set NODE_ENV=production').exec();

    gulp.src("webpack.config.js", { cwd: "../../FrontEnd" })
           .pipe(webpack(myConfig, function (err, stats) {
               if (err) throw new utility.PluginError("webpack:build", err);
               utility.log("[webpack:build]", stats.toString({
                   colors: true
               }));
           }));

    return gulp.src(['../../FrontEnd/build/app.js'])
    .pipe(gulp.dest("./Build"));
});