const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

var config = {
    entry: { 
        main: "./Resources/main.js"
    },
    output: {
        path: path.join(__dirname, "wwwroot", "dist"),
        publicPath: "/dist/",
        filename: "[name].js"
    },
    resolve: {
        extensions: [".js"]
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                use: {
                    loader: "babel-loader"
                }
            },
            {
                test: /\.s?[ac]ss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: "css-loader", options: { sourceMap: true } },
                    { loader: "postcss-loader" },
                    { loader: "sass-loader", options: { sourceMap: true } }
                ]

            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "main.css",
        })
    ]
};

module.exports = (env, argv) => {
    if (argv.mode === "development") {
        config.devtool = "source-map"; //generate source
    }

    return config;
};
