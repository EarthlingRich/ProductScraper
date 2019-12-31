module.exports = [
    {
        test: /\.(js|jsx)$/,
        use: [ 'babel-loader' ]
    },
    {
        test: /\.s(a|c)ss$/,
        use: [ 'style-loader', 'css-loader', 'sass-loader' ]
    },
];