using System.Security.Cryptography.X509Certificates;

namespace ShootingGalleryGame;

public class ShootingGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private bool _debugMode = false;

    private static Texture2D _targetSprite;
    private Texture2D _crosshairsSprite;
    private Texture2D _backgroundSprite;
    private SpriteFont _gameFont;

    private static Vector2 _targetPosition = new Vector2(300, 300);
    private const int TargetRadius = 45;
    private Random _random = new Random();

    private float _targetMaxHeight = 0f;
    private float _targetMaxWidth = 0f;

    MouseState _mState;
    bool _mReleased = true;
    int _score = 0;

    private static double _setTimer = 10;
    private double _timer = _setTimer;
    private bool _GameOverState = false;
    private double _cpm = 0;

    /// <summary>
    /// Game Constructor
    /// We put game settings in here.
    /// </summary>
    public ShootingGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// This occurs once the game starts. We use this to load components onto the game.
    /// </summary>
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _targetPosition = new Vector2(
            _random.Next(TargetRadius, GraphicsDevice.Viewport.Width - TargetRadius), 
            _random.Next(TargetRadius, GraphicsDevice.Viewport.Height - TargetRadius));

        base.Initialize();
    }

    /// <summary>
    /// We use this almost always. This is where we load images, sounds, and other content into our game.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _targetSprite = Content.Load<Texture2D>("target");
        _crosshairsSprite = Content.Load<Texture2D>("crosshairs");
        _backgroundSprite = Content.Load<Texture2D>("sky");

        _gameFont = Content.Load<SpriteFont>("galleryFont");
    }

    /// <summary>
    /// This is the game loop and runs every frame of the game. In MonoGame this runs by default at 60 fps.
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (Keyboard.GetState().IsKeyDown(Keys.R) && _GameOverState != false)
        {
            _timer = _setTimer;
            _GameOverState = false;
            _score = 0;
            _cpm = 0;
        }
        
        if (_timer < 0)
        {
            _GameOverState = true;
        }

        if (_GameOverState != true)
        {
            _timer -= gameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Start update logic here
            _mState = Mouse.GetState();
            // _targetMaxHeight = _targetPosition.Y + _targetSprite.Height;
            // _targetMaxWidth = _targetPosition.X + _targetSprite.Width;

            if (_mState.LeftButton == ButtonState.Pressed && _mReleased != false)
            {
                // Deprecated solution
                // if (CheckForHit(_targetPosition.X, _targetMaxWidth, _targetPosition.Y, _targetMaxHeight, _mState.Position))
                // {
                //     _score++;
                // }

                // Better solution
                float mouseTargetDistance = Vector2.Distance(_targetPosition, _mState.Position.ToVector2());
                if (mouseTargetDistance <= TargetRadius)
                {
                    _score++;
                }

                // Once attempt to hit the target change its current position
                _targetPosition = new Vector2(
                    _random.Next(TargetRadius, _graphics.PreferredBackBufferWidth - TargetRadius),
                    _random.Next(TargetRadius, _graphics.PreferredBackBufferHeight - TargetRadius));

                _mReleased = false;
            }

            if (_mState.LeftButton == ButtonState.Released && _mReleased != true)
            {
                _mReleased = true;
            }

            _cpm = _score / _setTimer;
        }

        base.Update(gameTime);
    }


    /// <summary>
    /// This section handles all forms of rendering to the screen. Similar to the update section where it
    /// runs once every frame, but this area is used sole for images and text. We do not perform calculations
    /// or update variables here. We handle those operations inside of update.
    /// </summary>
    /// <param name="gameTime"></param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Begin drawing code here
        // The order you draw sprites to the screen is important.
        _spriteBatch.Begin();
        _spriteBatch.Draw(_backgroundSprite, new Vector2(0, 0), Color.White);
        //_spriteBatch.DrawString(_gameFont, "Test message", new Vector2(100, 100), Color.White);
        _spriteBatch.DrawString(_gameFont, _score.ToString(), new Vector2(3, 3), Color.White);

        if (_GameOverState == false)
        {
            _spriteBatch.DrawString(_gameFont, $"Time Left: {_timer:N0}", new Vector2(3, 40), Color.White);
        }
        else
        {
            _spriteBatch.DrawString(_gameFont, "Time Left: 0", new Vector2(3, 40), Color.White);
        }
        
        _spriteBatch.DrawString(_gameFont, $"{_cpm}", new Vector2(3, 80), Color.White);

        if (_debugMode == true)
        {
            _spriteBatch.DrawString(_gameFont, _targetMaxHeight.ToString(), new Vector2(20, 100), Color.White);
            _spriteBatch.DrawString(_gameFont, _targetMaxWidth.ToString(), new Vector2(80, 100), Color.White);
            
            _spriteBatch.DrawString(_gameFont, _targetPosition.X.ToString(), new Vector2(20, 200), Color.White);
            _spriteBatch.DrawString(_gameFont, _targetPosition.Y.ToString(), new Vector2(80, 200), Color.White);
        }
        
        _spriteBatch.Draw(_targetSprite, new Vector2(_targetPosition.X - TargetRadius, _targetPosition.Y - TargetRadius), Color.White);
        
        _spriteBatch.End();
        
        
        
        

        base.Draw(gameTime);
    }
    
    
    // Deprecated
    private bool CheckForHit(float minX, float maxX, float minY, float maxY, Point currentMousePos)
    {
        return (currentMousePos.X >= minX && currentMousePos.X <= maxX) &&
               (currentMousePos.Y >= minY && currentMousePos.Y <= maxY);
    }
}